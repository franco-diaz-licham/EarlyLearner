using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EarlyLearner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "households",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_households", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "readiness_outcomes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    description = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: false),
                    category = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_readiness_outcomes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "carers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    role = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_carers", x => x.id);
                    table.ForeignKey(
                        name: "fk_carers_households_household_id",
                        column: x => x.household_id,
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "children",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    given_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_children", x => x.id);
                    table.ForeignKey(
                        name: "fk_children_households_household_id",
                        column: x => x.household_id,
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stored_files",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    storage_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    file_name = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    content_type = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    size_in_bytes = table.Column<long>(type: "bigint", nullable: false),
                    media_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    uploaded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stored_files", x => x.id);
                    table.ForeignKey(
                        name: "fk_stored_files_households_household_id",
                        column: x => x.household_id,
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "daily_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    child_id = table.Column<Guid>(type: "uuid", nullable: false),
                    log_date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_daily_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_daily_logs_children_child_id",
                        column: x => x.child_id,
                        principalTable: "children",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_daily_logs_households_household_id",
                        column: x => x.household_id,
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "goals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    child_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: false),
                    type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    timeframe_start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    timeframe_end_date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_goals", x => x.id);
                    table.ForeignKey(
                        name: "fk_goals_children_child_id",
                        column: x => x.child_id,
                        principalTable: "children",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_goals_households_household_id",
                        column: x => x.household_id,
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "learning_plans",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    child_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    period_end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    focus = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_learning_plans", x => x.id);
                    table.ForeignKey(
                        name: "fk_learning_plans_children_child_id",
                        column: x => x.child_id,
                        principalTable: "children",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_learning_plans_households_household_id",
                        column: x => x.household_id,
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "observations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    child_id = table.Column<Guid>(type: "uuid", nullable: false),
                    observed_on = table.Column<DateOnly>(type: "date", nullable: false),
                    note = table.Column<string>(type: "character varying(2400)", maxLength: 2400, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_observations", x => x.id);
                    table.ForeignKey(
                        name: "fk_observations_children_child_id",
                        column: x => x.child_id,
                        principalTable: "children",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_observations_households_household_id",
                        column: x => x.household_id,
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "portfolio_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    child_id = table.Column<Guid>(type: "uuid", nullable: false),
                    captured_on = table.Column<DateOnly>(type: "date", nullable: false),
                    caption = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: false),
                    source_source_type = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    source_evidence_record_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_source_date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_portfolio_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_portfolio_items_children_child_id",
                        column: x => x.child_id,
                        principalTable: "children",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_portfolio_items_households_household_id",
                        column: x => x.household_id,
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "readiness_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    child_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_readiness_profiles", x => x.id);
                    table.ForeignKey(
                        name: "fk_readiness_profiles_children_child_id",
                        column: x => x.child_id,
                        principalTable: "children",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_readiness_profiles_households_household_id",
                        column: x => x.household_id,
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "completed_activities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    daily_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_completed_activities", x => x.id);
                    table.ForeignKey(
                        name: "fk_completed_activities_daily_logs_daily_log_id",
                        column: x => x.daily_log_id,
                        principalTable: "daily_logs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "daily_log_stored_files",
                columns: table => new
                {
                    daily_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stored_file_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_daily_log_stored_files", x => new { x.daily_log_id, x.stored_file_id });
                    table.ForeignKey(
                        name: "fk_daily_log_stored_files_daily_logs_daily_log_id",
                        column: x => x.daily_log_id,
                        principalTable: "daily_logs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_daily_log_stored_files_stored_files_stored_file_id",
                        column: x => x.stored_file_id,
                        principalTable: "stored_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reading_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    daily_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: false),
                    author = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    child_response = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reading_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_reading_entries_daily_logs_daily_log_id",
                        column: x => x.daily_log_id,
                        principalTable: "daily_logs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "routine_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    daily_log_id = table.Column<Guid>(type: "uuid", nullable: false),
                    routine_name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    notes = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_routine_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_routine_entries_daily_logs_daily_log_id",
                        column: x => x.daily_log_id,
                        principalTable: "daily_logs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "goal_readiness_outcomes",
                columns: table => new
                {
                    goal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    readiness_outcome_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_goal_readiness_outcomes", x => new { x.goal_id, x.readiness_outcome_id });
                    table.ForeignKey(
                        name: "fk_goal_readiness_outcomes_goals_goal_id",
                        column: x => x.goal_id,
                        principalTable: "goals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_goal_readiness_outcomes_readiness_outcomes_readiness_outcom",
                        column: x => x.readiness_outcome_id,
                        principalTable: "readiness_outcomes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "planned_learning_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    learning_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    planned_date = table.Column<DateOnly>(type: "date", nullable: false),
                    title = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: false),
                    status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_planned_learning_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_planned_learning_sessions_learning_plans_learning_plan_id",
                        column: x => x.learning_plan_id,
                        principalTable: "learning_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "observation_readiness_outcomes",
                columns: table => new
                {
                    observation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    readiness_outcome_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_observation_readiness_outcomes", x => new { x.observation_id, x.readiness_outcome_id });
                    table.ForeignKey(
                        name: "fk_observation_readiness_outcomes_observations_observation_id",
                        column: x => x.observation_id,
                        principalTable: "observations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_observation_readiness_outcomes_readiness_outcomes_readiness",
                        column: x => x.readiness_outcome_id,
                        principalTable: "readiness_outcomes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "observation_stored_files",
                columns: table => new
                {
                    observation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stored_file_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_observation_stored_files", x => new { x.observation_id, x.stored_file_id });
                    table.ForeignKey(
                        name: "fk_observation_stored_files_observations_observation_id",
                        column: x => x.observation_id,
                        principalTable: "observations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_observation_stored_files_stored_files_stored_file_id",
                        column: x => x.stored_file_id,
                        principalTable: "stored_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "portfolio_item_readiness_outcomes",
                columns: table => new
                {
                    portfolio_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    readiness_outcome_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_portfolio_item_readiness_outcomes", x => new { x.portfolio_item_id, x.readiness_outcome_id });
                    table.ForeignKey(
                        name: "fk_portfolio_item_readiness_outcomes_portfolio_items_portfolio",
                        column: x => x.portfolio_item_id,
                        principalTable: "portfolio_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_portfolio_item_readiness_outcomes_readiness_outcomes_readin",
                        column: x => x.readiness_outcome_id,
                        principalTable: "readiness_outcomes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "portfolio_item_stored_files",
                columns: table => new
                {
                    portfolio_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stored_file_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_portfolio_item_stored_files", x => new { x.portfolio_item_id, x.stored_file_id });
                    table.ForeignKey(
                        name: "fk_portfolio_item_stored_files_portfolio_items_portfolio_item_",
                        column: x => x.portfolio_item_id,
                        principalTable: "portfolio_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_portfolio_item_stored_files_stored_files_stored_file_id",
                        column: x => x.stored_file_id,
                        principalTable: "stored_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "readiness_outcome_progresses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    readiness_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    readiness_outcome_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_readiness_outcome_progresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_readiness_outcome_progresses_readiness_outcomes_readiness_o",
                        column: x => x.readiness_outcome_id,
                        principalTable: "readiness_outcomes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_readiness_outcome_progresses_readiness_profiles_readiness_p",
                        column: x => x.readiness_profile_id,
                        principalTable: "readiness_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "suggested_next_steps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    readiness_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    readiness_outcome_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_suggested_next_steps", x => x.id);
                    table.ForeignKey(
                        name: "fk_suggested_next_steps_readiness_outcomes_readiness_outcome_id",
                        column: x => x.readiness_outcome_id,
                        principalTable: "readiness_outcomes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_suggested_next_steps_readiness_profiles_readiness_profile_id",
                        column: x => x.readiness_profile_id,
                        principalTable: "readiness_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "completed_activity_readiness_outcomes",
                columns: table => new
                {
                    completed_activity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    readiness_outcome_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_completed_activity_readiness_outcomes", x => new { x.completed_activity_id, x.readiness_outcome_id });
                    table.ForeignKey(
                        name: "fk_completed_activity_readiness_outcomes_completed_activities_",
                        column: x => x.completed_activity_id,
                        principalTable: "completed_activities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_completed_activity_readiness_outcomes_readiness_outcomes_re",
                        column: x => x.readiness_outcome_id,
                        principalTable: "readiness_outcomes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "completed_activity_stored_files",
                columns: table => new
                {
                    completed_activity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stored_file_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_completed_activity_stored_files", x => new { x.completed_activity_id, x.stored_file_id });
                    table.ForeignKey(
                        name: "fk_completed_activity_stored_files_completed_activities_comple",
                        column: x => x.completed_activity_id,
                        principalTable: "completed_activities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_completed_activity_stored_files_stored_files_stored_file_id",
                        column: x => x.stored_file_id,
                        principalTable: "stored_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reading_entry_stored_files",
                columns: table => new
                {
                    reading_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stored_file_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reading_entry_stored_files", x => new { x.reading_entry_id, x.stored_file_id });
                    table.ForeignKey(
                        name: "fk_reading_entry_stored_files_reading_entries_reading_entry_id",
                        column: x => x.reading_entry_id,
                        principalTable: "reading_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reading_entry_stored_files_stored_files_stored_file_id",
                        column: x => x.stored_file_id,
                        principalTable: "stored_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "routine_entry_stored_files",
                columns: table => new
                {
                    routine_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stored_file_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_routine_entry_stored_files", x => new { x.routine_entry_id, x.stored_file_id });
                    table.ForeignKey(
                        name: "fk_routine_entry_stored_files_routine_entries_routine_entry_id",
                        column: x => x.routine_entry_id,
                        principalTable: "routine_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_routine_entry_stored_files_stored_files_stored_file_id",
                        column: x => x.stored_file_id,
                        principalTable: "stored_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "planned_learning_session_goals",
                columns: table => new
                {
                    planned_learning_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    goal_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_planned_learning_session_goals", x => new { x.planned_learning_session_id, x.goal_id });
                    table.ForeignKey(
                        name: "fk_planned_learning_session_goals_goals_goal_id",
                        column: x => x.goal_id,
                        principalTable: "goals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_planned_learning_session_goals_planned_learning_sessions_pl",
                        column: x => x.planned_learning_session_id,
                        principalTable: "planned_learning_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "planned_learning_session_readiness_outcomes",
                columns: table => new
                {
                    planned_learning_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    readiness_outcome_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_planned_learning_session_readiness_outcomes", x => new { x.planned_learning_session_id, x.readiness_outcome_id });
                    table.ForeignKey(
                        name: "fk_planned_learning_session_readiness_outcomes_planned_learnin",
                        column: x => x.planned_learning_session_id,
                        principalTable: "planned_learning_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_planned_learning_session_readiness_outcomes_readiness_outco",
                        column: x => x.readiness_outcome_id,
                        principalTable: "readiness_outcomes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "evidence_references",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    readiness_outcome_progress_id = table.Column<int>(type: "integer", nullable: false),
                    readiness_outcome_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_type = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    evidence_record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    observed_on = table.Column<DateOnly>(type: "date", nullable: false),
                    summary = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_evidence_references", x => x.id);
                    table.ForeignKey(
                        name: "fk_evidence_references_readiness_outcome_progress_readiness_ou",
                        column: x => x.readiness_outcome_progress_id,
                        principalTable: "readiness_outcome_progresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_evidence_references_readiness_outcomes_readiness_outcome_id",
                        column: x => x.readiness_outcome_id,
                        principalTable: "readiness_outcomes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_carers_household_id_user_id",
                table: "carers",
                columns: new[] { "household_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_children_household_id_given_name",
                table: "children",
                columns: new[] { "household_id", "given_name" });

            migrationBuilder.CreateIndex(
                name: "ix_completed_activities_daily_log_id",
                table: "completed_activities",
                column: "daily_log_id");

            migrationBuilder.CreateIndex(
                name: "ix_completed_activity_readiness_outcomes_readiness_outcome_id",
                table: "completed_activity_readiness_outcomes",
                column: "readiness_outcome_id");

            migrationBuilder.CreateIndex(
                name: "ix_completed_activity_stored_files_stored_file_id",
                table: "completed_activity_stored_files",
                column: "stored_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_daily_log_stored_files_stored_file_id",
                table: "daily_log_stored_files",
                column: "stored_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_daily_logs_child_id",
                table: "daily_logs",
                column: "child_id");

            migrationBuilder.CreateIndex(
                name: "ix_daily_logs_household_id_child_id_log_date",
                table: "daily_logs",
                columns: new[] { "household_id", "child_id", "log_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_evidence_references_readiness_outcome_id",
                table: "evidence_references",
                column: "readiness_outcome_id");

            migrationBuilder.CreateIndex(
                name: "ix_evidence_references_readiness_outcome_progress_id",
                table: "evidence_references",
                column: "readiness_outcome_progress_id");

            migrationBuilder.CreateIndex(
                name: "ix_goal_readiness_outcomes_readiness_outcome_id",
                table: "goal_readiness_outcomes",
                column: "readiness_outcome_id");

            migrationBuilder.CreateIndex(
                name: "ix_goals_child_id",
                table: "goals",
                column: "child_id");

            migrationBuilder.CreateIndex(
                name: "ix_goals_household_id_child_id_status",
                table: "goals",
                columns: new[] { "household_id", "child_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_learning_plans_child_id",
                table: "learning_plans",
                column: "child_id");

            migrationBuilder.CreateIndex(
                name: "ix_learning_plans_household_id_child_id",
                table: "learning_plans",
                columns: new[] { "household_id", "child_id" });

            migrationBuilder.CreateIndex(
                name: "ix_observation_readiness_outcomes_readiness_outcome_id",
                table: "observation_readiness_outcomes",
                column: "readiness_outcome_id");

            migrationBuilder.CreateIndex(
                name: "ix_observation_stored_files_stored_file_id",
                table: "observation_stored_files",
                column: "stored_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_observations_child_id",
                table: "observations",
                column: "child_id");

            migrationBuilder.CreateIndex(
                name: "ix_observations_household_id_child_id_observed_on",
                table: "observations",
                columns: new[] { "household_id", "child_id", "observed_on" });

            migrationBuilder.CreateIndex(
                name: "ix_planned_learning_session_goals_goal_id",
                table: "planned_learning_session_goals",
                column: "goal_id");

            migrationBuilder.CreateIndex(
                name: "ix_planned_learning_session_readiness_outcomes_readiness_outco",
                table: "planned_learning_session_readiness_outcomes",
                column: "readiness_outcome_id");

            migrationBuilder.CreateIndex(
                name: "ix_planned_learning_sessions_learning_plan_id_planned_date",
                table: "planned_learning_sessions",
                columns: new[] { "learning_plan_id", "planned_date" });

            migrationBuilder.CreateIndex(
                name: "ix_portfolio_item_readiness_outcomes_readiness_outcome_id",
                table: "portfolio_item_readiness_outcomes",
                column: "readiness_outcome_id");

            migrationBuilder.CreateIndex(
                name: "ix_portfolio_item_stored_files_stored_file_id",
                table: "portfolio_item_stored_files",
                column: "stored_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_portfolio_items_child_id",
                table: "portfolio_items",
                column: "child_id");

            migrationBuilder.CreateIndex(
                name: "ix_portfolio_items_household_id_child_id_captured_on",
                table: "portfolio_items",
                columns: new[] { "household_id", "child_id", "captured_on" });

            migrationBuilder.CreateIndex(
                name: "ix_readiness_outcome_progresses_readiness_outcome_id",
                table: "readiness_outcome_progresses",
                column: "readiness_outcome_id");

            migrationBuilder.CreateIndex(
                name: "ix_readiness_outcome_progresses_readiness_profile_id_readiness",
                table: "readiness_outcome_progresses",
                columns: new[] { "readiness_profile_id", "readiness_outcome_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_readiness_outcomes_category_sort_order",
                table: "readiness_outcomes",
                columns: new[] { "category", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_readiness_outcomes_code",
                table: "readiness_outcomes",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_readiness_profiles_child_id",
                table: "readiness_profiles",
                column: "child_id");

            migrationBuilder.CreateIndex(
                name: "ix_readiness_profiles_household_id_child_id",
                table: "readiness_profiles",
                columns: new[] { "household_id", "child_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reading_entries_daily_log_id",
                table: "reading_entries",
                column: "daily_log_id");

            migrationBuilder.CreateIndex(
                name: "ix_reading_entry_stored_files_stored_file_id",
                table: "reading_entry_stored_files",
                column: "stored_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_routine_entries_daily_log_id",
                table: "routine_entries",
                column: "daily_log_id");

            migrationBuilder.CreateIndex(
                name: "ix_routine_entry_stored_files_stored_file_id",
                table: "routine_entry_stored_files",
                column: "stored_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_stored_files_household_id_storage_key",
                table: "stored_files",
                columns: new[] { "household_id", "storage_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_suggested_next_steps_readiness_outcome_id",
                table: "suggested_next_steps",
                column: "readiness_outcome_id");

            migrationBuilder.CreateIndex(
                name: "ix_suggested_next_steps_readiness_profile_id",
                table: "suggested_next_steps",
                column: "readiness_profile_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "carers");

            migrationBuilder.DropTable(
                name: "completed_activity_readiness_outcomes");

            migrationBuilder.DropTable(
                name: "completed_activity_stored_files");

            migrationBuilder.DropTable(
                name: "daily_log_stored_files");

            migrationBuilder.DropTable(
                name: "evidence_references");

            migrationBuilder.DropTable(
                name: "goal_readiness_outcomes");

            migrationBuilder.DropTable(
                name: "observation_readiness_outcomes");

            migrationBuilder.DropTable(
                name: "observation_stored_files");

            migrationBuilder.DropTable(
                name: "planned_learning_session_goals");

            migrationBuilder.DropTable(
                name: "planned_learning_session_readiness_outcomes");

            migrationBuilder.DropTable(
                name: "portfolio_item_readiness_outcomes");

            migrationBuilder.DropTable(
                name: "portfolio_item_stored_files");

            migrationBuilder.DropTable(
                name: "reading_entry_stored_files");

            migrationBuilder.DropTable(
                name: "routine_entry_stored_files");

            migrationBuilder.DropTable(
                name: "suggested_next_steps");

            migrationBuilder.DropTable(
                name: "completed_activities");

            migrationBuilder.DropTable(
                name: "readiness_outcome_progresses");

            migrationBuilder.DropTable(
                name: "observations");

            migrationBuilder.DropTable(
                name: "goals");

            migrationBuilder.DropTable(
                name: "planned_learning_sessions");

            migrationBuilder.DropTable(
                name: "portfolio_items");

            migrationBuilder.DropTable(
                name: "reading_entries");

            migrationBuilder.DropTable(
                name: "routine_entries");

            migrationBuilder.DropTable(
                name: "stored_files");

            migrationBuilder.DropTable(
                name: "readiness_outcomes");

            migrationBuilder.DropTable(
                name: "readiness_profiles");

            migrationBuilder.DropTable(
                name: "learning_plans");

            migrationBuilder.DropTable(
                name: "daily_logs");

            migrationBuilder.DropTable(
                name: "children");

            migrationBuilder.DropTable(
                name: "households");
        }
    }
}
